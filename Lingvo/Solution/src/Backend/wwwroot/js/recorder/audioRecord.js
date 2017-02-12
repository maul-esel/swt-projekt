var audio_context = null;
var meter = null;
var canvasContext = null;
var WIDTH=100;
var HEIGHT=20;
var rafID = null;
var mediaStreamSource = null;

(function(window){

  var WORKER_PATH = '/js/recorder/recorderWorker.js';


	function RecorderObject (source, cfg){

		var config = cfg || {};
		var recording = false, initialized=false,
			currCallback, worker;

		var bufferLen = config.bufferLen || 4096;
		this.context = source.context;
		this.node = (this.context.createScriptProcessor ||
		this.context.createJavaScriptNode).call(this.context,
			bufferLen, 2, 2);
		worker = new Worker(config.workerPath || WORKER_PATH);
		var mp3LibPath = config.mp3LibPath || 'lame.all.js';
		

		worker.onmessage = function(e){
			var blob = e.data;
			currCallback(blob);
		};

		worker.postMessage({
			command: 'init',
			config: {
				sampleRate: this.context.sampleRate,
				mp3LibPath: mp3LibPath,
				
				recordAsMP3: config.recordAsMP3 || false
				
			}
		});



		this.configure = function(cfg){
			for (var prop in cfg){
				if (cfg.hasOwnProperty(prop)){
					config[prop] = cfg[prop];
				}
			}
		};


		this.record = function(){
			recording = true;
		};

		this.stop = function(){
			recording = false;
		};

		this.clear = function(){
			worker.postMessage({ command: 'clear' });
		};

		this.getBuffer = function(cb) {
			currCallback = cb || config.callback;
			worker.postMessage({ command: 'getBuffer' })
		};

		this.exportWAV = function(cb, type){
			currCallback = cb || config.callback;
			type = type || config.type || 'audio/wav';
			if (!currCallback) throw new Error('Callback not set');

			worker.postMessage({
				command: 'exportWAV',
				type: type
			});
		};

		this.exportOGG = function(cb){

			currCallback = cb || config.callback;
			console.log("making this call");
			worker.postMessage({
				command: 'exportOGG'
			});
			
		};


		this.exportMP3 = function(cb){
			currCallback = cb || config.callback;
			console.log("making this call");
			worker.postMessage({
				command: 'exportMP3'
			});

		};



		this.node.onaudioprocess = function(e){
			if (!recording) return;


			worker.postMessage({
				command: 'record',
				buffer: [
					e.inputBuffer.getChannelData(0),
				]
			});
		};

		source.connect(this.node);
		this.node.connect(this.context.destination);    //this should not be necessary




	}



  var audioRecorder =  {

	  fromSource: function(src, cfg){
		 return new RecorderObject(src, cfg);
	  },

	  requestDevice: function (callback, cfg) {

		  cfg = cfg || {};
		  callback = callback || function(){};

		  window.AudioContext = window.AudioContext || window.webkitAudioContext;
		  navigator.getUserMedia = ( navigator.getUserMedia ||
		  navigator.webkitGetUserMedia ||
		  navigator.mozGetUserMedia ||
		  navigator.msGetUserMedia);
		  window.URL = window.URL || window.webkitURL;

		  audio_context = new AudioContext;


		  navigator.getUserMedia(
			  {
				  "audio": {
                		"mandatory": {
							"googEchoCancellation": "true",
							"googAutoGainControl": "true",
							"googNoiseSuppression": "true",
							"googHighpassFilter": "false"
                	},
                	"optional": []
				},
            }, function(stream){

			  callback(new RecorderObject(audio_context.createMediaStreamSource(stream), cfg));
			  createVisualizer(stream);

		  }, function(e) {

			  console.log("An error occurred"); //Null if something goes wrong
			  callback(null);

		  });

	  }


	};


	window.audioRecorder = audioRecorder;






})(window);


function didntGetStream() {
    alert('Stream generation failed.');
}


function createVisualizer(stream) {
	canvasContext = document.getElementById( "meter" ).getContext("2d");
    // Create an AudioNode from the stream.
    mediaStreamSource = audio_context.createMediaStreamSource(stream);

    // Create a new volume meter and connect it.
    meter = createAudioMeter(audio_context);
    mediaStreamSource.connect(meter);

    // kick off the visual updating
    drawLoop();
}

function drawLoop( time ) {
    // clear the background
    canvasContext.clearRect(0,0,WIDTH,HEIGHT);
    canvasContext.fillStyle = "blue";

    // draw a bar based on the current volume
    canvasContext.fillRect(0, 0, meter.volume*WIDTH*3.5, HEIGHT);

    // set up the next visual callback
    rafID = window.requestAnimationFrame( drawLoop );
}
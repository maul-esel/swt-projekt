
  var elapsed_time = 0;
  var elapsed_time_display;
  var audio_context;
  var recorder;
  var current_recording;
  var recording = 0;
  
  function startRecording(button) {
    recording = recording + 1;
    recorder.clear();
    recorder && recorder.record();
    button.disabled = true;
    button.nextElementSibling.disabled = false;
    resetElapsedTime()
    elapsed_time_display = setInterval(displayElapsedTime,1000);
  }
  
  function stopRecording(button) {
    recorder && recorder.stop();
    button.disabled = true;
    button.previousElementSibling.disabled = false;
    clearInterval(elapsed_time_display)
    prepareRecording();
  }
  
  function displayElapsedTime() {
    
    $("#seconds").html(padTimeCode(elapsed_time++%60));
    $("#minutes").html(padTimeCode(parseInt(elapsed_time/60,10)));
    
  }

  function resetElapsedTime() {
    elapsed_time = 0;
    displayElapsedTime();
  }

  function prepareRecording() {
    recorder.exportMP3(function(blob){
        current_recording = blob;
        setNewRecordingDisplay(URL.createObjectURL(blob));
      //Functions for displaying the current recording could be called here
      //Playback function could be set up here
    });
  }

  function sendBlobToServer(event) {
        if(current_recording == null){
          return;
      }
      $("#submit-modal").modal()
      event.preventDefault()
       
        var form = $("#pageForm")[0];
        var action = form.getAttribute("action")

        var formData = new FormData(form);
        formData.append("RecordedFile", current_recording);
        var request = new XMLHttpRequest();

        request.onload = () => {
            $("#submit-modal").modal("hide")
        }

        request.open("POST", action);
        request.send(formData);
        return false;
  }
  
  window.onload = function init() {
    audioRecorder.requestDevice(function(recorderObject){
      recorder = recorderObject;
    });
    };

    function setNewRecordingDisplay(blobUrl)
    {
        var now = new Date();
        $("#newRecordingName").attr("value", "Aufgenommen um " + now.getHours() + ":" + now.getMinutes() + " Uhr");
        $("#newRecordingAudio").attr("src", blobUrl)
        $("#newRecordingDownload").attr('href', blobUrl)
        $("#noNewRecordingWarning").hide();
        $("#newRecording").removeClass('hidden')
    }

    function setUploadedFileDisplay()
    {
        //$("#newRecordingName").attr("value", 
    }

    function padTimeCode ( val ) {
     return val > 9 ? val : "0" + val; 
   }
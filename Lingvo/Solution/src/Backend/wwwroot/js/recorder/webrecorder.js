
  var elapsed_time = 0;
  var elapsed_time_display;
  var on_air_display;
  var audio_context;
  var recorder;
  var current_recording;
  var recording = 0;
  var isSubmit = false;
  var isCancel = false;

  function toggleRecording(toggle) {
      if (toggle.checked) {
          startRecording()
      } else {
          stopRecording()
      }
  }

  function startRecording() {
    recording = recording + 1;
    recorder.clear();
    recorder && recorder.record();
    resetElapsedTime()
    elapsed_time_display = setInterval(displayElapsedTime,1000);
    on_air_display = setInterval(displayOnAir,1000);
  }
  
  function stopRecording() {
    recorder && recorder.stop();
    clearInterval(elapsed_time_display)
    clearInterval(on_air_display);
    resetDisplayOnAir();
    prepareRecording();
  }
  
  function displayElapsedTime() {
    
    $("#seconds").html(padTimeCode(elapsed_time++%60));
    $("#minutes").html(padTimeCode(parseInt(elapsed_time/60,10)));
    
  }

  function displayOnAir() {
    var dotCount = elapsed_time%4;
    $("#onair").html("Aufnahme läuft" + ".".repeat(dotCount));
  }

  function resetElapsedTime() {
    elapsed_time = 0;
    displayElapsedTime();
  }

  function resetDisplayOnAir() {
    $("#onair").html("");
  }

  function prepareRecording() {
    $("#conversion-modal").modal()
    recorder.exportMP3(function(blob){
        current_recording = blob;
        setNewRecordingDisplay(blob);
        $("#conversion-modal").modal("hide")
    });
  }

    function leavePage(event) {
        
        isCancel = true
    
    }


   function sendBlobToServer(event) {
      $("#submit-modal").modal()
      event.preventDefault()
       
        var form = $("#pageForm")[0];
        var action = form.getAttribute("action")

        var formData = new FormData(form);

        if (current_recording != null) {
            formData.append("RecordedFile", current_recording);
        }

        const duration = Math.ceil($("#newRecordingAudio")[0].duration * 1000)
        formData.append("Duration", duration)

        // ugly hack for accessing xhr and responseURL
        var xhr;
        var _orgAjax = jQuery.ajaxSettings.xhr;
        jQuery.ajaxSettings.xhr = function () {
            xhr = _orgAjax();
            return xhr;
        };

        $.ajax({
            url: action,
            processData: false,
            contentType: false,
            data: formData,
            method: 'POST'
        })

        .done(() => {
            $("#submit-modal").modal("hide")
            window.location.replace(xhr.responseURL)
        })

        .fail((jq, s, err) => {
            $("#submit-modal").modal("hide")
            $("#submit-error-modal").modal()
        })
       isSubmit = true
  }
  
   window.onload = function init() {
       $("#record-btn-toggle").attr('checked', false)
    audioRecorder.requestDevice(function(recorderObject){
      recorder = recorderObject;
    });
  };

    function setNewRecordingDisplay(blob)
    {
        const now = new Date();
        const name = "Aufgenommen um " + padTimeCode(now.getHours()) + ":" + padTimeCode(now.getMinutes()) + " Uhr"
        updateNewTrackUI(name, blob)
    }

    function updateNewTrackUI(name, blob) {
        var blobURL = URL.createObjectURL(blob)

        $("#newRecordingName").val(name)

        $("#newRecordingAudio").attr("src", blobURL)
        $("#newRecordingDownload").attr('href', blobURL)

        if (!name.endsWith(".mp3"))
            name = name + ".mp3"
        $("#newRecordingDownload").attr("download", name)

        $("#noNewRecordingWarning").hide()
        $("#newRecording").removeClass("hidden")
    }

    function padTimeCode ( val ) {
     return val > 9 ? val : "0" + val; 
   }

    window.onbeforeunload = function() {
        if (!isSubmit && current_recording != null || !isCancel && !isSubmit) {
            return "Die erstellte Aufnahme wurde noch nicht gespeichert. Möchten Sie diese Seite wirklich verlassen?";
        }
    }

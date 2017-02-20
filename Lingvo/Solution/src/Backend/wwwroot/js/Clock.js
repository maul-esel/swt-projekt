   

  var elapsed_time = 0;
  var elapsed_time_display;
  var on_air_display;


function padTimeCode ( val ) {
    
    return val > 9 ? val : "0" + val; 
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
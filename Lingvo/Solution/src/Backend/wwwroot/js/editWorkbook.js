var userWantsToLeave = false;

function leavePage(event) {
    
    userWantsToLeave = true
    
}

window.onbeforeunload = function() {
    if (!userWantsToLeave) {
        return "Die erstellte Aufnahme wurde noch nicht gespeichert. Möchten Sie diese Seite wirklich verlassen?";
    }
}

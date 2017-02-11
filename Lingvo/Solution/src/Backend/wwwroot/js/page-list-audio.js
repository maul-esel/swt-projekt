$(() => {
    var playingAudios = []
    const resetPlayButton = (audio, link) => {
        audio.pause()
        audio.currentTime = 0
        $(".glyphicon", link).addClass("glyphicon-play-circle").removeClass("glyphicon-stop")
    }
    $(".page-play-button").click((event) => {
        const link = $(event.currentTarget)
        const container = link.parent()
        const audio = $(".page-play-audio", container)[0]

        if (audio.currentTime === 0) {
            audio.play()
            $(audio).on('ended', () => resetPlayButton(audio, link))
            $(".glyphicon", link).addClass("glyphicon-stop").removeClass("glyphicon-play-circle")
        } else
            resetPlayButton(audio, link)
    })
})
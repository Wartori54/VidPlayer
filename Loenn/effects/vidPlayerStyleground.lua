local vidPlayerStyleground = {}

vidPlayerStyleground.name = "VidPlayer/VidPlayerStyleground"

vidPlayerStyleground.canBackground = true
vidPlayerStyleground.canForeground = true
vidPlayerStyleground.defaultData = {
    video = "",
    muted = true,
    keepAspectRatio = true,
    volumeMult = 1,
}

vidPlayerStyleground.fieldInformation = {
    volumeMult = {
        fieldType = "number",
        minimumValue = 0
    }
}

return vidPlayerStyleground
local vidPlayerStyleground = {}

vidPlayerStyleground.name = "VidPlayer/VidPlayerStyleground"

vidPlayerStyleground.canBackground = true
vidPlayerStyleground.canForeground = true
vidPlayerStyleground.defaultData = {
    video = "",
    muted = true,
    keepAspectRatio = true,
    volumeMult = 1,
    globalAlpha = 1,
    centered = false,
    chromaKey = "",
    entityChromaKeyTolAbs = 0,
    entityChromaKeyTolRel = 0,
}

vidPlayerStyleground.fieldInformation = {
    volumeMult = {
        fieldType = "number",
        minimumValue = 0
    },
    globalAlpha = {
        fieldType = "number",
        minimumValue = 0,
        maximumValue = 1,
    },
    chromaKey = {
        fieldType = "color",
        allowEmpty = true,
    },
    entityChromaKeyTolAbs = {
        fieldType = "number"
    },
    entityChromaKeyTolRel = {
        fieldType = "number"
    }
}

return vidPlayerStyleground
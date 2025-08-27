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
    chromaKeyBaseThr = 0,
    chromaKeyAlphaCorr = 0.1,
    chromaKeySpill = 0.1,
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
    chromaKeyBaseThr = {
        fieldType = "number",
    },
    chromaKeyAlphaCorr = {
        fieldType = "number",
        minimumValue = 0.000001, -- epsilon
    },
    chromaKeySpill = {
        fieldType = "number",
        minimumValue = 0.000001, -- epsilon
    },
}

return vidPlayerStyleground
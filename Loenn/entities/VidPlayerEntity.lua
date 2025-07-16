local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local vidPlayerEntity = {}
vidPlayerEntity.name = "VidPlayer/VidPlayerEntity"
vidPlayerEntity.depth = -1000000
vidPlayerEntity.canResize = {true, true}
vidPlayerEntity.placements = {
    name = "vidPlayerEntity",
    placementType = "point",
    data = {
        width = 64,
        height = 36,
        video = "",
        muted = false,
        keepAspectRatio = true,
        looping = true,
        hires = true,
        volumeMult = 1,
        globalAlpha = 1,
        centered = false,
    }
}
vidPlayerEntity.fieldInformation = {
    volumeMult = {
        fieldType = "number",
        minimumValue = 0
    },
    globalAlpha = {
        fieldType = "number",
        minimumValue = 0,
        maximumValue = 1,
    }
}

function vidPlayerEntity.sprite(room, entity)
    local rect = drawableRectangle.fromRectangle("bordered", entity.x, entity.y, entity.width, entity.height,  {0, 0, 0, 0.5}, "Black")
    local icon = drawableSprite.fromTexture("VidPlayerEntity/play_icon", entity)
    icon:addPosition(entity.width/2, entity.height/2)
    
    return {rect, icon}
end


return vidPlayerEntity
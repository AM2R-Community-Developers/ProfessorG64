var mirrorAngle;
//Boolean in equation, multiple cases
//((image_xscale == -1) * 180) Replaced by new variable, `mirrorAngle`
mirrorAngle = 0
if (image_xscale == -1)
    mirrorAngle = 180
if (oControl.mod_fusion == 1)
{
    switch global.currentsuit
    {
        case 0:
            draw_sprite_ext(sSpaceJumpTrail_fusion, 0, x, y, image_xscale, 1, (image_angle + mirrorAngle), c_white, image_alpha)
            break
        case 1:
            draw_sprite_ext(sSpaceJumpTrailVaria_fusion, 0, x, y, image_xscale, 1, (image_angle + mirrorAngle), c_white, image_alpha)
            break
        case 2:
            draw_sprite_ext(sSpaceJumpTrailGravity_fusion, 0, x, y, image_xscale, 1, (image_angle + mirrorAngle), c_white, image_alpha)
            break
    }

}
else
{
    switch global.currentsuit
    {
        case 0:
            draw_sprite_ext(sSpaceJumpTrail, 0, x, y, image_xscale, 1, (image_angle + mirrorAngle), c_white, image_alpha)
            break
        case 1:
            draw_sprite_ext(sSpaceJumpTrailVaria, 0, x, y, image_xscale, 1, (image_angle + mirrorAngle), c_white, image_alpha)
            break
        case 2:
            draw_sprite_ext(sSpaceJumpTrailGravity, 0, x, y, image_xscale, 1, (image_angle + mirrorAngle), c_white, image_alpha)
            break
    }

}

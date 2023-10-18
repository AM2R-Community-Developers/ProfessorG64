var jawdir, jawreverse;
//Original code with boolean in equation
//var jawdir = direction + 270 + (image_yscale == -1) * 180
//patched code
jawreverse = 0
if (sign(image_yscale) == -1)
    jawreverse = -180
jawdir = ((direction + 270) + jawreverse)
//end patch
draw_sprite_ext(sErisJaw, -1, (x + lengthdir_x(jawoff, jawdir)), (y + lengthdir_y(jawoff, jawdir)), 1, image_yscale, image_angle, -1, 1)
draw_sprite_ext(sErisHead, -1, x, y, 1, image_yscale, image_angle, -1, 1)
if (flashing > 0)
{
    draw_set_blend_mode(bm_add)
    repeat (2)
    {
        draw_sprite_ext(sErisJaw, -1, (x + lengthdir_x(jawoff, jawdir)), (y + lengthdir_y(jawoff, jawdir)), 1, image_yscale, image_angle, -1, (flashing * 0.1))
        draw_sprite_ext(sErisHead, -1, x, y, 1, image_yscale, image_angle, -1, (flashing * 0.1))
    }
    draw_set_blend_mode(bm_normal)
}
if boosting
{
    draw_set_blend_mode(bm_add)
    if (fxtimer == 0 || fxtimer == 1)
    {
        draw_sprite_ext(sErisJaw, -1, (x + lengthdir_x(jawoff, jawdir)), (y + lengthdir_y(jawoff, jawdir)), 1, image_yscale, image_angle, c_green, (1.1 + (fxtimer * 0.2)))
        draw_sprite_ext(sErisHead, -1, x, y, 1, image_yscale, image_angle, c_green, (1.1 + (fxtimer * 0.2)))
    }
    if (fxtimer == 2 || fxtimer == 3)
    {
        repeat (2)
        {
            draw_sprite_ext(sErisJaw, -1, (x + lengthdir_x(jawoff, jawdir)), (y + lengthdir_y(jawoff, jawdir)), 1, image_yscale, image_angle, c_yellow, 1)
            draw_sprite_ext(sErisHead, -1, x, y, 1, image_yscale, image_angle, c_yellow, 1)
            draw_sprite_ext(sErisJaw, -1, (x + lengthdir_x(jawoff, jawdir)), (y + lengthdir_y(jawoff, jawdir)), 1, image_yscale, image_angle, c_white, 1)
            draw_sprite_ext(sErisHead, -1, x, y, 1, image_yscale, image_angle, c_white, 1)
        }
    }
    if (fxtimer == 4 || fxtimer == 5)
    {
        repeat (2)
        {
            draw_sprite_ext(sErisJaw, -1, (x + lengthdir_x(jawoff, jawdir)), (y + lengthdir_y(jawoff, jawdir)), 1, image_yscale, image_angle, c_red, 1)
            draw_sprite_ext(sErisHead, -1, x, y, 1, image_yscale, image_angle, c_red, 1)
        }
    }
    draw_set_blend_mode(bm_normal)
}

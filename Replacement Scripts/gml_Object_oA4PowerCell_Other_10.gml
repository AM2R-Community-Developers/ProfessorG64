global.event[203] = 1
image_index = 1
repeat (10)
{
    expl = instance_create((x + random_range(-30, 30)), (y + random_range(-30, 30)), oFXAnimSpark)
    expl.image_speed = 0.5
    expl.additive = 0
    expl.sprite_index = sExplSmoke
    expl.depth = -90
    //Original code with boolean in equation
    //expl.image_xscale = 1 - 2 * (random(2) < 1)
    //patched code
    expl.image_xscale = choose(-1, 1)
    //end patch
}
instance_create(0, 0, oA4EscapeControl)

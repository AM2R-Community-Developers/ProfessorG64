event_inherited()
image_speed = 0
tileid = -1
tilesandcrn0 = -1
tilesandcrn1 = -1
tilesandcrn2 = -1
tilesandcrn3 = -1
pos = ((string(x) + ",") + string(y))
if (room != rm_a3h04)
    event_user(1)
else
    alarm[2] = 1
// Set object's depth to default Foreground value instead of -101. Otherwise sand corners won't be visible after the change to tile depths in alarm 1 and 2
depth = -100
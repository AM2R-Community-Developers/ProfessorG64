var list, str_list;
list = ds_list_create()
i = 0
ds_list_add(list, global.mod_fusion_unlocked)
repeat (19)
{
    ds_list_add(list, 0)
    i += 1
}
str_list = ds_list_write(list)
ds_list_clear(list)
ds_list_destroy(list)
return str_list;

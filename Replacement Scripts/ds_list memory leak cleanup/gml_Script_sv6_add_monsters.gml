var list, str_list;
list = ds_list_create()
i = 0
repeat (100)
{
    ds_list_add(list, global.metdead[i])
    i += 1
}
str_list = ds_list_write(list)
ds_list_clear(list)
ds_list_destroy(list)
return str_list;

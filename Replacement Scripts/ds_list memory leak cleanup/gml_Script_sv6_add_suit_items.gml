var list, str_list;
list = ds_list_create()
ds_list_add(list, global.item[0])
ds_list_add(list, global.item[1])
ds_list_add(list, global.item[2])
ds_list_add(list, global.item[3])
ds_list_add(list, global.item[4])
ds_list_add(list, global.item[5])
ds_list_add(list, global.item[6])
ds_list_add(list, global.item[7])
ds_list_add(list, global.item[8])
ds_list_add(list, global.item[9])
ds_list_add(list, global.item[10])
ds_list_add(list, global.item[11])
ds_list_add(list, global.item[12])
ds_list_add(list, global.item[13])
ds_list_add(list, global.item[14])
str_list = ds_list_write(list)
ds_list_clear(list)
ds_list_destroy(list)
return str_list;

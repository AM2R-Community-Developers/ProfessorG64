list = ds_list_create()
ds_list_read(list, base64_decode(file_text_read_string(argument0)))
ds_list_clear(list)
ds_list_destroy(list)

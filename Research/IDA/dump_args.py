res = []
for xref in CodeRefsTo(get_screen_ea(), 1): 
    args = idaapi.get_arg_addrs(xref)
    if args:
        arg_offset = args[1]
        str_data = idc.get_strlit_contents(str_offset)
        if str_data:
            res.append(str_data.decode())
res = list(dict.fromkeys(res))
print(",".join(res))
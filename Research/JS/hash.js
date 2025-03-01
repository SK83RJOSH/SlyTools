function hash_string(str)
{
    let result = 0;

    for (i = 0; i < str.length; ++i)
    {
        let c = str.charCodeAt(i);

        if (c >= 97 && c <= 122)
        {
            c -= 32;
        }

        result = (((result * 31) | 0) + c) | 0;
    }

    return result;
}

function line(str)
{
    var hash = hash_string(str);
    return str + "," + hash + "," + "0x" + (hash>>>0).toString(16).padStart(8, "0");
}
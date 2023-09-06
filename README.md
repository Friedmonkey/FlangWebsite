# FlangWebsite
a website made with c# which allows for flang code

its really scuffed please do NOT use it


# how to use
make a file in the `www` directory ending in `.flang` (for example index.flang) use `<(flang` to open a flang section and use `)>` to close it
then use `print` to append that line

`www/index.flang`
```
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Welcome!</title>
</head>
<body>
<h1>My Website!</h1>
<(flang
print("<h3>Hello world</h3>");
)>
</body>
</html>
```
# default varibles
- `PAGE` the current page
- `GET["varible"]` url querry string
- `POST["varible"]` xxx url encoded form data
- `CONTENT` xxx url encoded form data but as Raw

if you just want to print something you can use `<(="Hello World")>`
```
<title><(="Welcome to {PAGE}"$)></title>
```


example of a comment section
`www/comments.flang`
```
<!DOCTYPE html>
<(flang
string genTag(string tag,string text)
{
	return "<{tag}>{text}</{tag}>"$;
}
)>

<html>

<head>
    <meta charset="utf-8" />
    <title>Comments</title>
</head>
<body>
<(flang
	if (!File.exists("comments.csv"))
		File.write("comments.csv","FriedMonkey,hello monkeys!\n");
	string file = File.read("comments.csv");
	
	var username = POST["username"];
	var content = POST["content"];
	
	if (username != null && content != null)
	{
		username = username.replace(",","%comma%").replace("\n","%enter%");
		content = content.replace(",","%comma%").replace("\n","%enter%");
		file += "{username},{content}\n"$;
		File.write("comments.csv",file);
	}
	file = File.read("comments.csv");
	var lines = file.split("\n");
	foreach(var line in lines)
	{
		string user = line.split(",").first().replace("%comma%",",").replace("%enter%","\n");
		string cont = line.split(",").last().replace("%comma%",",").replace("%enter%","\n");
		print(genTag("h3",user));
		print(genTag("h5",cont));
	}
	
	
)>
<h3>Write your own comment!</h3>
<form method="post" action="comments.flang">

    <input type="text" name="username" placeholder="Me">
    <input type="text" name="content" placeholder="Hi guys">
    <input type="submit" style="margin-top: 5px; border-top: 2px solid black;">
</form>
<script>
if ( window.history.replaceState ) {
  window.history.replaceState( null, null, window.location.href );
}
</script>
</body>
</html>
```

# fallback

in the root folder `/www`
you can put a `fallback.flang`

so if the user visits any webpage that doest exist itll redirect that that
example of `www/fallback.flang`
```
<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8" />
    <title>Fallback page</title>
</head>
<body>
<h1>The page "<(=PAGE)>" does not exist so enjoy this image instead!</h1>
<img src="^root/cool.jpg">
</body>
</html>
```


here you can see i use `^` in a path, this means that it is a rooted path, everything before and the `^` self gets discarded
this is quite usefull because when you are in the `fallback.flang` you can be from anywhere so using `^` is almost nessesery in a fallback page



lets say we have the image `/www/root/cool.jpg`
now if we are at `/www/site/data/index.flang` we would either have to backtrace or do it some other way
but in `/www/site/data/index.flang` we can simply call `^root/cool.jpg`

this also means that
`^root/cool.jpg`
and
`/www/test/anything/file.zip/test/img.txt^root/cool.jpg`

are both going to
`/www/root/cool.jpg`

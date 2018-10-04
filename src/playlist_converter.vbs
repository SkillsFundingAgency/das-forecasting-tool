Set fso = CreateObject("Scripting.FileSystemObject")
Set regexp = CreateObject("VBScript.RegExp")

Set args=Wscript.Arguments

Dim playlist, path, content, pattern

pattern = "<Add Test=""(.*?)"" \/>"
playlist=args(0)
path="Playlists\" + playlist
WScript.echo "Trying to read file from: " + path

Set file = fso.OpenTextFile(path, 1)
content = file.ReadAll

WScript.echo "File was uploaded. Parsing.." 

regexp.Pattern = pattern
regexp.Global = True

Set matches = regexp.Execute(content)

WScript.echo "File was parsed. Total tests: " + cstr(matches.count)

Set textStream = fso.CreateTextFile("testlist", True)

For Each match In matches
	textStream.WriteLine(match.SubMatches(0))
Next

WScript.echo "Test list saved to file testlist"









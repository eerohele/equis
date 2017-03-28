Equis
=====

Watch an XSLT stylesheet and a directory of source files for changes.

When the stylesheet changes, transform all source files.

When a source file changes, transform only that file.

## Use

1. [Download](https://github.com/eerohele/equis/releases).
2. Run. Example:

```
C:\> Path\To\Equis.exe watch --stylesheet My-Awesome-Stylesheet.xsl --source My-Awesome-Input-Directory
[15:16:50 INF] Watching My-Awesome-Stylesheet.xsl and My-Awesome-Input-Directory/*.xml for changes.
```

To transform once instead of watching files for changes, use
`Equis.exe transform ...` instead.

Equis
=====

Watch an XSLT 1.0 stylesheet and a directory of source files for changes.

When the stylesheet changes, transform all source files.

When a source file changes, transform only that file.

Optionally, validate both the source and target files against an XML schema.

## Use

1. [Download](https://github.com/eerohele/equis/releases).
2. Run. Example:

```
C:\> Path\To\Equis.exe watch --stylesheet My-Awesome-Stylesheet.xsl --source My-Awesome-Input-Directory --output-schema My-Awesome-Xml-Schema.xsd
[15:16:50 INF] Watching My-Awesome-Stylesheet.xsl and My-Awesome-Input-Directory/*.xml for changes.
```

To transform once instead of watching files for changes, use
`Equis.exe transform ...` instead.

For more information, see `Equis.exe watch --help` and `Equis.exe transform --help`.

## Limitations
- XSLT 1.0 only, because that's what I'm stuck with.

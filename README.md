<img src="https://raw.githubusercontent.com/BinaryKits/ZPLUtility/master/doc/logo.png" width="200">

# BinaryKits.Zpl.net45

This project is a fork of https://github.com/BinaryKits/BinaryKits.Zpl that is modified to work with the .Net Framework 4.5 legacy platform.

---

The main differences between the versions are:
* `SkiaSharp Version 1.68.3` instead of `SkiaSharp Version 2.80.3`
    * SKFonts not supported, removed the code for them.
* `Magick.NET-Q8-x86 Version 11.1.0` instead of `SixLabors.ImageSharp 1.0.4`
* `Microsoft.Extensions.Logging.Abstractions` removed entirely.
  
  ---

  You can see the modifications that were made [in this commit](https://github.com/Tilation/BinaryKits.Zpl.net45/commit/84e8cc5dc90e600f4f9205bbc9f789ed855ef9bb).
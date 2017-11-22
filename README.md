# bilinear-interpolation-wpf-cpp
### This repo provides an example for this stackoverflow question.
https://stackoverflow.com/questions/47416295/is-there-a-chance-to-make-the-bilinear-interpolation-faster?noredirect=1#comment81814838_47416295

#### Tools
The project is a Visual Studio 2017 project.

* BilinearInterpolationMVP is the main application that calls the MediaLibraryProxy
* MediaLibraryProxy acts as an interface for the main application and the c++ part. It is needed because it contains unsafe code.
* MediaLibrary containts the c++ code with the Resize / Merge / Blend function.

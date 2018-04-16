# WpfDrawPerformanceTest
Search for fast drawing method with WPF

Measure FPS by writing drawing processing for each type of DrawType

[Blog http://puni-o.hatenablog.com/entry/2018/04/11/135728](http://puni-o.hatenablog.com/entry/2018/04/11/135728)

# Contents

Place the control in the MainWindow of WPF and measure the drawing speed of that control.


## Method

Draw a lot of particles(short lines) with a pen whose color changes every frame (palette is 8 colors).  
The movement of Particles and the creation of pallet are the same.  
Change the drawing part only for each DrawType and look at FPS.

## Type of DrawType

DrawType is defined as enum

### None

It does not draw.  
I think this is the maximum FPS.

### NotFreeze

For checking how late it is when Freezable is not Freeze.  
Drawing Particles one at a time in DrawingContext.DrawLine.

### Freeze

Almost the same as NotFreeze.  
Check the speed when Freeze.

### Grouping

Particle's drawing is done by DrawingContext.DrawGeometry in palette unit.

### BackingStore

Drawing in the DrawingGroup and drawing it in DrawingContext.DrawDrawing collectively.

### WriteableBitmap

Using WriteableBitmap (Since it is troublesome to manipulate WriteableBitmap directly, I use WriteableBitmapEx).


# Result

CPU : Core i7-6700 , GPU : GeForce GTX 1070  
(I do not know if the measurement method is fair)

| DrawType | 10000 particles FPS(roughly) | 50000 |
---- | ---- | ----
| NotFreeze | very slow | |
| Freeze | 50 fps | 10 fps |
| Grouping | 90 fps | 20 fps |
| BackingStore | 10 fps | very slow |
| WriteableBitmap | 120 fps | 95 fps |




Please tell me a nice idea :)
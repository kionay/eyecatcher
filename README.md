# Eyecatcher

### Generation Methods

##### Random
Random generation creates 100 lines with random starting points, distances, and directions.

The lines are snapped to the angle in the Angle dropdown.\*

##### Grid-Erase
The grid-erase method generates an entire grid throughout the canvas. 
90% of lines are removed, creating a pattern of sorts.

The initial grid should\* allow angles 60, 90, and 120 for each of the 3 
[euclidean regular tilings](https://en.wikipedia.org/wiki/Euclidean_tilings_by_convex_regular_polygons#Regular_Tilings)
(triangular, square, and hexagonal tiling respectively.)\*

\* - The dropdown currently only contains a single item. The reason for this is that the Random generation method, and previous unsatisfactory generation methods used this dropdown more appropriately. However, I don't currently have a way to create the grid-erase triangular and hexagonal tilings without custom-writing each method. I had hoped to be able to generate the tiling given only the angle, but that may not be where this program goes in the future. 

### Notes

* Adjustable canvas size
	* Issue: Canvas needs to be clipped appropriately upon resizing.
* Saving canvas as a PNG image

* Importing and exporting raw line data
	* This could benefit from compression and optimization routines.

* Parallelism and multithreading could be used if the needed canvas size became very large and we needed to keep the UI responsive.


This program was made with the intention of "creating images that might catch one's eye." With the intent to create images that looked interesting I seem to have accidentally fell into the field of [Tessellation](https://en.wikipedia.org/wiki/Tessellation).

I'm not sure if I will put more effort into this, as applications currently exist for creating tessellations. They're more advanced and wholesome than what I have here, but I'm putting this on github in case anyone finds the code or methods I used here of any use.

Some snippets of code came from the Internet (mostly where my mathematics knowledge failed me) and I have tried to give proper attribution in the comments of the code to where I got those snippets from. For those functions the credit goes to their owners and the wonderful website of stackoverflow.
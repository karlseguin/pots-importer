# Points of Interest Imporer #

This is the importer used to generate the files at <http://data.mongly.com>. 

The data comes from <http://wiki.openstreetmap.org/>, and is available under [Creative Commons Attribution-ShareAlike 2.0](http://creativecommons.org/licenses/by-sa/2.0/) license.

# More Info #

You can see a demo at <http://pots.mongly.com>. You can read more at <http://openmymind.net/2011/6/20/MongoDB--OpenStreetMap-and-a-Little-Demo>

# Data #

This importer generates more data than what is available to download.  It tracks the original node id (nid) or way id (wid) that was used to generate our version of the node. The idea is that these could be used when applying a diff.
# Carta Mei

Welcome to Carta Mei!

Carta Mei is a tool to create layered maps. For example, you'd have the shore lines on the first layer, then your trip on the second layer, and finally some names of countries and cities on the third layer.

## [Milestones](https://github.com/lverre/CartaMei/milestones)

### Milestone 1

Milestone 1 is all that I need to do what I am designing CartaMei in the first place :)

* read and display shorelines, rivers, lakes and borders (from the NOAA's [GSHHG](https://www.ngdc.noaa.gov/mgg/shorelines/gshhs.html) data) - [issue #1](https://github.com/lverre/CartaMei/issues/1)
* offer different Mercator projection - [issue #2](https://github.com/lverre/CartaMei/issues/2)
* display your trips - [issue #3](https://github.com/lverre/CartaMei/issues/3)
* display custom places (cities in particular) - [issue #4](https://github.com/lverre/CartaMei/issues/4)
* export to images - [issue #5](https://github.com/lverre/CartaMei/issues/5)
* special backgrounds - [issue #6](https://github.com/lverre/CartaMei/issues/6):
** images (allows for scanned maps)
** parchment look (this might be pushed to v2 since it kind of can be done using Photoshop / Gimp and use an image background)
** watermark (this might be pushed to v2 since I don't need it myself)

### Future Work

* export to animated GIFs - [issue #7](https://github.com/lverre/CartaMei/issues/7)
* terrain displayer (levels of color, contour lines for elevation, mountains like in Tolkien maps, etc.) - [issue #8](https://github.com/lverre/CartaMei/issues/8)
* map maker (create and edit maps on the GSHHG format) - [issue #9](https://github.com/lverre/CartaMei/issues/9)

## Technologies

Carta Mei is written in C# 6 using the .NET Framework 4.6. It also uses:
* WPF for the UI
* [MEF](http://mef.codeplex.com/) for the plugin architecture
* [Avalon Dock](http://wpftoolkit.codeplex.com/) for the main interface

The solution is a Visual Studio (Express) 2015 solution.

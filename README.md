Introduction
============

Rewrote Fandro in C#. Will search. BMH.

Fandro2 ("nee, Fandro Winforms")
============

I've been working on this version since 2011 or so - but I finally had time to sort of 'wrap it up'.

Why use it?
============

No idea. You can search files while also filtering out your files based on specific FileInfo filters? You want to learn some BMH?

Features
============

* Fast multi-threaded search for text in files with optional FileInfo filters (creation, access, mod date and file size - more coming!)
* Multi folder selection mode - select which folders you want to search thru/filter thru

Tech features:
============

* Non recursive file search
* Self-written components for datagrid ("controlgrid")
* Removed a lot of windows specific stuff

Future
============

* Separate out BMH routines.
* Fandro.Terminal/CLI 
* Multiplatform (low priority because of the crazy amount of "desktop frameworks" for .Net)

Multiplatform?
=============

Fandro at this stage is a good test bed to experiment with all these other UI frameworks available for .Net

Screenshots:
============

Single folder mode

<img
src="images/main_screen_singlefolder_mode.png"
raw=true
alt="Fandro Single folder mode"
/>

Multi-folder mode

<img
src="images/main_screen_multifolder_mode.png"
raw=true
alt="Fandro Multi folder mode"
/>

Filter screen

<img
src="images/main_screen_filters_working.png"
raw=true
alt="Fandro "
/>

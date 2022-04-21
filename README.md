# DiskReader
A small tool to make backups of whole Physical Disks to file `(.img, .bin etc)`

### Usage:
Open Administrator Command Prompt and type:

`diskreader.exe \\.\PhysicalDisk1 E:\Images\Disk1.bin`

List connected disks:  

`diskreader.exe ListDisks`

`diskreader.exe Volumes`


### Notes:

- I haven't tested restoring the backed up images, but they successfully mount in OSFMount.
- Read progress isn't supported currently, I am looking to change this.

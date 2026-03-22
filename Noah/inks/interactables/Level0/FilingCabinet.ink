INCLUDE ../../globals.ink

-> main_menu

=== main_menu ===
+ [Organize files] -> organize_files
+ [Back] -> END


=== organize_files ===
+ [A-Z] 
    -> organize_az
+ [Z-A] 
    -> organize_za
+ [Back] 
    -> back_from_organize

=== back_from_organize ===
-> main_menu

=== organize_az ===
MC $$$ Ah, as it should be.
-> main_menu

=== organize_za ===
MC $$$ We're going reverse...
-> main_menu

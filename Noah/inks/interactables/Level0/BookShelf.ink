INCLUDE ../../globals.ink

-> main_menu

VAR book_1 = false
VAR book_2 = false
VAR book_3 = false
VAR book_4 = false
VAR book_5 = false
VAR book_6 = false
VAR book_7 = false

=== main_menu ===
+ [Search for some books] -> search

+ [Back] -> END

=== search ===
// If we have seen all three specific lines, show the default
{ book_1 && book_2 && book_3 && book_4 && book_5 && book_6 && book_7:
    I guess that's all the books.
    -> main_menu
}

// Otherwise, shuffle between the ones we haven't seen
{~
    - -> book1
    - -> book2
    - -> book3
    - -> book4
    - -> book5
    - -> book6
    - -> book7
}
-> main_menu


=== book1 ===
Book $$$ A Field Guide to Identifying Different Shades of Beige.
MC $$$ I always thought the cubicle walls were 'Sad Sand,' but according to Chapter 3, they're actually 'Despairful cream'.

~ book_1 = true
-> main_menu

=== book2 ===
Book $$$ Why Your Printer Hates You (And How to Apologize).
MC $$$ Apparently, if I whisper a haiku to the paper tray, it might stop double-printing the invoices? Honestly, at this point, I'm willing to try anything.
~ book_2 = true
-> main_menu

=== book3 ===
Book $$$ The Collected Poetry of a Sentient Excel Spreadsheet.
MC $$$ This hits different on a Monday. I'm actually starting to feel bad about that 'Clear All' I did earlier.
~ book_3 = true
-> main_menu

=== book4 ===
Book $$$ Aggressive Stapling: A Manifestation Guide.
MC $$$ The thunk-clack sound is actually the universe saying 'Yes.'
~ book_4 = true
-> main_menu

=== book5 ===
Book $$$ 101 Ways to Look Busy While Dissociating.
MC $$$ The 'Intense Squint at a Blank Outlook Calendar' technique is brilliant. I've been doing the 'Rapid Mouse Clicking' for years, but this book says that's an amateur move.
~ book_5 = true
-> main_menu

=== book6 ===
Book $$$ The Secret Language of Passive-Aggressive Post-it Notes.
MC $$$ I need to check the exact angle of the one Sam left on my monitor this morning; if it's tilted ten degrees to the left, I think he's technically challenged me to a duel in the parking lot.
~ book_6 = true
-> main_menu

=== book7 ===
Book $$$ Why the Last Hour of the Day Is the Longest.
MC $$$ The book says time slows in proportion to your awareness of it. That explains a lot.
~ book_7 = true
-> main_menu

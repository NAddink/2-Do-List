INCLUDE ../globals.ink

MC $$$ This floor is very floor
I sure hope this floor has a lot of floors.

Floor $$$ Hey diva.
I'm the floor.
[shake rate=20.0 level=5 connected=1]I hope u like my floor-ness...[/shake]

{selected_choice_3 == false && selected_choice_4 && selected_choice_5:
    -> AFTER_CHOICE
- else:
    -> FIRST_TIME
}

=== FIRST_TIME ===
How many choices should I give you?
* [3 choices maybe?] -> 3Choices
* [Let's do 4 choices.] -> 4Choices
* [5 choices. Keep em comin'] -> 5Choices

=== 3Choices ===

Three? Make your choice then.
~ selected_choice_3 = true
* [Here's a choice] -> AFTER_CHOICE
* [This is another choice] -> AFTER_CHOICE
* [A trio of choices?] -> AFTER_CHOICE

=== 4Choices ===

Four? Ok a nice even number. What do you pick?
~ selected_choice_4 = true
* [This is an option] -> AFTER_CHOICE
* [Two options is better than one] -> AFTER_CHOICE
* [Three options! Fun.] -> AFTER_CHOICE
* [Four? A bit much.] -> AFTER_CHOICE

=== 5Choices ===
~ selected_choice_5 = true
Five???? Wow Mr. Moneybags over here.
* [One big boom] -> AFTER_CHOICE
* [Two big booms] -> AFTER_CHOICE
* [Three big booms] -> AFTER_CHOICE
* [Four big booms] -> AFTER_CHOICE
* [Five big booms!!!] -> AFTER_CHOICE

=== AFTER_CHOICE === 

Nice.
-> END
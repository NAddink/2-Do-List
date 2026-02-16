INCLUDE ../../../globals.ink

/* Vars in use:
day0_office_TalkedTosam
day0_office_HelpedJanice

*/
-> JaniceStart

=== JaniceStart ===
{
    - day0_office_TalkedToSam:
        -> JaniceMain
    - else:
        -> JaniceFlavorText
}

=== JaniceFlavorText ===
MC $$$ Janice seems a bit overwhelmed at the moment.
-> END

=== JaniceMain ===
MC $$$ Hey Janice, Sam said you might need some help?
Janice $$$ Oh thank goodness! Just the person I needed!
I can't remember how to open this excel file, and I figure you're so good with computers. Can you help me out here?

+ [Sure, Janice.] -> JaniceHelp
+ [No] -> JaniceNo


=== JaniceNo ===
Janice $$$ Oh... I can try to figure it out myself I suppose... thanks anyways.
-> END

=== JaniceHelp ===
Janice $$$ Oh I just knew you would help me!
Such a kind young man. Not like Sam of course, that lazy bum never gives me the time of day!
~ day0_office_HelpedJanice = true
-> END
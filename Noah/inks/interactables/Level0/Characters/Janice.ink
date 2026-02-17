INCLUDE ../../../globals.ink

/* Vars in use:
day0_office_TalkedTosam
day0_office_HelpedJanice

*/
-> Start

=== Start ===
{
    - day0_office_HelpedJanice:
        -> Thanks
    - day0_office_TalkedToSam:
        -> SamHelp
    - else:
        -> FlavorText
}

=== Thanks ===
Thanks again for the help! Such a sweet young man you are. If only everybody was so patient with me.
-> END

=== SamHelp ===
MC $$$ Hey Janice, Sam said you might need some help?
-> Main


=== FlavorText ===
MC $$$ Janice seems a bit overwhelmed at the moment.
-> Main

=== Main ===
Janice $$$ Oh thank goodness! Just the person I needed!
I can't remember how to open this excel file, and I figure you're so good with computers. Can you help me out here?

+ [Sure, Janice. You just click here.] -> Help
+ [No] -> No


=== No ===
Janice $$$ Oh... I can try to figure it out myself I suppose... thanks anyways.
-> END

=== Help ===
Janice $$$ Oh I just knew you would help me!
Such a kind young man. Not like Sam of course, that lazy bum never gives me the time of day!
~ day0_office_HelpedJanice = true
-> END
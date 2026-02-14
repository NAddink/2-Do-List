INCLUDE ../../../globals.ink

-> SamStart

=== SamStart ===
{
    - day0_office_TalkedToSam:
        -> SamMain
    - else:
        -> SamInitial
}


=== SamMain ===
Sam $$$ Bro do you have the attention span of a gnat or something?
Did you already forget everything I just said to you?
+ [Enlighten me...] -> SamRepeat
+ [No] -> END

-> END

=== SamRepeat === 
I SAID that bossman told you to reschedule the shareholder meeting. I said I would do it for you but in exchange, you need to go help Janice with her claims.
That woman drives me up the wall I swear...
-> END

=== SamInitial ===
Sam $$$ Hey bossman told you to reschedule that shareholder meeting happening later today didn't't he? I'll tell you what, Janice needs some help with her claims, you do that and I'll handle this scheduling mess. I can't deal with that woman today.
~ day0_office_TalkedToSam = true
-> END
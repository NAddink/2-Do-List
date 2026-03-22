INCLUDE ../../../globals.ink

-> Start

=== Start ===
{
    - day0_office_TalkedToSam:
        -> Main
    - else:
        -> Initial
}


=== Main ===
Sam $$$ Bro do you have the attention span of a gnat or something?
Did you already forget everything I just said to you?
+ [Enlighten me...] -> Repeat
+ [No] -> END

-> END

=== Repeat === 
Sam $$$ I SAID that bossman told you to reschedule the shareholder meeting. I said I would do it for you but in exchange, you need to go help Janice with her claims.
That woman drives me up the wall I swear...
-> END

=== Initial ===
Sam $$$ Hey bossman told you to reschedule that shareholder meeting happening later today didn't he? 
I'll tell you what, Janice needs some help with her claims, you do that and I'll handle this scheduling mess. 
I can't deal with that woman today.
~ day0_office_TalkedToSam = true

{
    -  day0_office_HelpedJanice:
        -> AlreadyHelpedJaniceChoice
    - else:
        -> END
}

=== AlreadyHelpedJaniceChoice ===
+ [I already helped Janice] -> AlreadyHelpedJanice
+ [Leave] -> END

=== AlreadyHelpedJanice ===
Sam $$$ Ok? Do you want an award or something?
At least she won't come harrassing me for help. I'll get around to rescheduling the meeting for you.
You're welcome.
-> END
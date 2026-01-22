INCLUDE ../../globals.ink

-> main_menu

=== main_menu ===
+ [Check Emails] -> check_emails
+ [Research Insurance News] -> research_news
+ [Update Client Records] -> update_records
+ [Exit] -> END

=== check_emails ===

+ [Respond to emails]
    -> respond_emails
+ [Clear spam]
    -> clear_spam
+ [Back]
    -> back_from_emails

=== respond_emails ===

No sir your insurance does not cover you lighting yourself on fire for a football party, send.

-> check_emails

=== clear_spam ===

DIE SPAM DIEEEEE

-> check_emails

=== back_from_emails ===

See you tomorrow junk mail.

-> main_menu

=== research_news ===

+ [Should the insurance industry be concerned with AI?]
    -> ai_concern
+ [Recent studies have shown that insurance companies are stealing your money.]
    -> stealing_money
+ [Back]
    -> back_from_research

=== ai_concern ===

Dang Robots

-> research_news

=== stealing_money ===

Well Obviously.

-> research_news

=== back_from_research ===

That’s enough research for today, or a lifetime.

-> main_menu

=== update_records ===

+ [Grace Monos]
    -> grace_monos
+ [Joshua Alvarez]
    -> joshua_alvarez
+ [Back]
    -> main_menu

=== grace_monos ===
Lets see here... ASU student... journalism... yep checks out.
-> update_records

=== joshua_alvarez ===
Is this file just the lyrics of every Sabrina Carpenter song???
-> update_records

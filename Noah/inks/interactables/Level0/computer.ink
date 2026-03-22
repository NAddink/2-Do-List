INCLUDE ../../globals.ink

-> main_menu

=== main_menu ===
+ {not (day0_office_ClearedSpam && day0_office_RespondedToEmails) }[Check Emails] -> check_emails
+ [Research Insurance News] -> research_news
+ [Update Client Records] -> update_records
+ [Back] -> END

=== check_emails ===

+ { not day0_office_RespondedToEmails } [Respond to emails]
    -> respond_emails
+ { not day0_office_ClearedSpam } [Clear spam]
    -> clear_spam
+ [Back]
    -> back_from_emails

=== respond_emails ===

MC $$$ No sir your insurance does not cover you lighting yourself on fire for a football party, send.
~ day0_office_RespondedToEmails = true
-> check_emails

=== clear_spam ===

MC $$$ DIE SPAM DIEEEEE
~ day0_office_ClearedSpam = true

-> check_emails

=== back_from_emails ===

MC $$$ See you tomorrow junk mail.

-> main_menu

=== research_news ===

+ [News article on AI]
    -> ai_concern
+ [News article on insurance companies]
    -> stealing_money
+ [Back]
    -> back_from_research

=== ai_concern ===

News $$$ "Should the insurance industry be concerned with AI?"
MC $$$ Dang Robots
~ day0_office_ResearchedInsurance = true

-> research_news

=== stealing_money ===

News $$$ "Recent studies have shown that insurance companies are stealing your money."
MC $$$ Well Obviously.
~ day0_office_ResearchedInsurance = true

-> research_news

=== back_from_research ===

MC $$$ That's enough research for today, or a lifetime.

-> main_menu

=== update_records ===

+ [Grace Monos]
    -> grace_monos
+ [Joshua Alvarez]
    -> joshua_alvarez
+ [Back]
    -> main_menu

=== grace_monos ===
MC $$$ Lets see here... ASU student... journalism... yep checks out.
~ day0_office_UpdatedRecords = true
-> update_records

=== joshua_alvarez ===
MC $$$ Is this file just the lyrics of every Sabrina Carpenter song???
~ day0_office_UpdatedRecords = true
-> update_records

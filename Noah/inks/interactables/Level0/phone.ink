INCLUDE ../../globals.ink

-> main_menu

=== main_menu ===
+ [Check Messages] -> check_messages
+ [Cut off dead-end clients] -> dead_end
+ [Call Potential Clients] -> call_clients
+ [Back] -> END

=== check_messages ===

Answering Machine $$$ ***BEEEEEEP*** Hey! This is Boss! Make yourself useful and grab me a coffee would ya? Stupid intern messed up my order...
-> main_menu

=== dead_end ===

+ [Sympathetic]
    -> dead_end_sympathetic
+ [Witty]
    -> dead_end_witty
+ [Back]
    -> main_menu

=== dead_end_sympathetic ===

MC $$$ I’m really sorry to have to do this ma’am but we are no longer able to offer you support.
-> dead_end

=== dead_end_witty ===
MC $$$ I forgot my wallet at my place, I'll pay for the next one.
-> dead_end

=== call_clients ===
MC $$$ Hi sir, we’d love to welcome you to the General Insurance family, No worries at a low price!
~ day0_office_CalledClients = true
-> main_menu
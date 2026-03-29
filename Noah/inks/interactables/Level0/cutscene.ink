INCLUDE ../../globals.ink

Narrator $$$ You are back in your cubicle doing work when the phone goes off. It's the boss again..
Boss $$$ GET IN HERE... [color=red][shake rate=20.0 level=5 connected=1]NOW![/shake][/color]
Narrator $$$ You quickly make your way to Boss' office
Boss $$$ Reschedule the shareholder meeting.
Boss $$$ Reschedule the shareholder meeting...
Boss $$$ [color=webmaroon][shake rate=20.0 level=15 connected=1]RESCHEDULE THE DAMN SHAREHOLDER'S MEETING.[/shake][/color]
{day0_office_MadeCoffeeGood: Boss $$$ AND TO THINK I THOUGHT YOU MIGHT BE USEFUL FOR SOMETHING}
{day0_office_MadeCoffeeBad: Boss $$$ I SHOULD'VE KNOWN BETTER, YOU CAN'T EVEN MAKE A DECENT CUP OF JOE}

Boss $$$ [color=webmaroon][shake rate=20.0 level=15 connected=1]YOU KNOW HOW MANY YEARS I'VE BEEN KISSING UP TO THESE PEOPLE? 23 STINKING YEARS JUST FOR YOU TO RUIN ALL MY HARD WORK.[/shake][/color]

+ [Blame Sam]
    -> blame_sam
+ [Take Ownership]
    -> take_ownership

=== blame_sam === 
MC $$$ Sam was supposed to do that. He said he would take care of it if I helped Janice with her claims. 
MC $$$ I'm sorry sir, it'll never happen again.
Boss $$$ You're right, it won't happen again. 
Boss $$$ You're [shake rate=20.0 level=5 connected=1]Fired[/shake]. 
Boss $$$ I have no need for an employee who can't do their own work and is willing to throw their peers under the bus to save their own skin. 
Boss $$$ Get out of my office.
-> END

=== take_ownership ===
MC $$$ I'm sorry sir, I forgot to write it on my to do list, it was my responsibility and I let you and the company down.
Boss $$$ I appreciate you taking ownership but that doesn't fix your mistake..
Boss $$$ I have to let you go, pack your stuff and get out of here.
-> END
INCLUDE ../../../globals.ink

/* Vars in use:
day0_office_TalkedToBoss
day0_office_GaveBossCoffee
day0_office_MadeCoffeeBad
day0_office_MadeCoffeeGood

*/

-> BossStart

=== BossStart ===
{
    - day0_office_GaveBossCoffee:
        -> END
    - day0_office_TalkedToBoss:
        -> BossMain
    - else:
        -> BossInitial
}

=== BossInitial ===

{
    - day0_office_MadeCoffeeGood || day0_office_MadeCoffeeBad:
        + [Give Coffee] -> GiveCoffee
        + [Nevermind...] -> END
    - else:
        -> InitialNoCoffee
}

=== InitialNoCoffee ===
Boss $$$ Did you not get the message I left on your phone? I wanted my coffee.
~ day0_office_TalkedToBoss = true
-> END


=== BossMain ===
{
    - day0_office_MadeCoffeeGood || day0_office_MadeCoffeeBad:
        + [Give Coffee] -> GiveCoffee
        + [Nevermind...] -> END
    - else:
        -> NoCoffee
}

=== NoCoffee === 
Boss $$$ So do you have my coffee or are you just trying to waste my time?
-> END

=== GiveCoffee ===
{
    - day0_office_MadeCoffeeGood:
        -> CoffeeGood
    - day0_office_MadeCoffeeBad:
        -> CoffeeBad
}

=== CoffeeGood ===
Boss $$$ Wow this is surprisingly good. You were a barista in your past life huh?
A bonus is coming your way next pay cycle.

~ day0_office_GaveBossCoffee = true
-> END

=== CoffeeBad ===
Boss $$$ Oh god this tastes like trash, have you never made coffee before?

~ day0_office_GaveBossCoffee = true
-> END
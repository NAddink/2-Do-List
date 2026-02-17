INCLUDE ../../globals.ink

VAR correct_base = false
VAR correct_milk = false
VAR correct_flavor = false
VAR correct_mix = false
VAR correct_topping = false

{
    - day0_office_MadeCoffeeBad || day0_office_MadeCoffeeGood:
        -> END
    - day0_office_StartedCoffee:
        -> Main
    - else:
        -> Initial
}

=== Initial ===
I can't forget what kind of coffee boss man likes!
He always gets an [color=green]iced americano[/color] with [color=green]almond milk[/color], [color=green]macha powder[/color] and he gets it [color=green]blended[/color] with a [color=green]caramel[/color] drizzle..
-> Main

=== Main ===
Ok... let's see here...

+ [Iced Coffee]
    ~ correct_base = false
    -> Milk
+ [Iced Americano]
    ~ correct_base = true
    -> Milk
+ [Cold Brew]
    ~ correct_base = false
    -> Milk


=== Milk ===

+ [Soy Milk]
    ~ correct_milk = false
    -> Flavor
+ [Oat Milk]
    ~ correct_milk = false
    -> Flavor
+ [Almond Milk]
    ~ correct_milk = true
    -> Flavor

=== Flavor ===

+ [Matcha Powder]
    ~ correct_flavor = true
    -> Mix
+ [Chai Powder]
    ~ correct_flavor = false
    -> Mix
+ [Mocha Powder]
    ~ correct_flavor = false
    -> Mix

=== Mix ===

+ [Blended]
    ~ correct_mix = true
    -> Topping
+ [Shaken]
    ~ correct_mix = false
    -> Topping
+ [Stirred]
    ~ correct_mix = false
    -> Topping

=== Topping ===

+ [Honey Drizzle]
    ~ correct_topping = false
    -> Finished
+ [Chocolate Drizzle]
    ~ correct_topping = false
    -> Finished
+ [Caramel Drizzle]
    ~ correct_topping = true
    -> Finished

=== Finished ===
{
    - correct_base && correct_milk && correct_flavor && correct_mix && correct_topping:
        ~ day0_office_MadeCoffeeGood = true
        MC $$$ I think that's everything. I hope he likes it.
        -> END
    - else:
        ~ day0_office_MadeCoffeeBad = true
        MC $$$ I think that's everything. I hope he likes it.
        -> END
}
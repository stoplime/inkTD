
        _____       _ _______ _____  
       |_   _|     | |__   __|  __ \ 
         | |  _ __ | | _| |  | |  | |
         | | | '_ \| |/ / |  | |  | |
        _| |_| | | |   <| |  | |__| |
       |_____|_| |_|_|\_\_|  |_____/ 
 
                v0.2

Team Members: Taylor, Steffen, Jackson

# Lighting

The basic design for the lighting system was a main directional sunlight, along with smaller point lights that radiate from the towers or scenery.

Our key light will be the Sun
The fill light will be the point lights on the towers
And the rim light will be the ambient light coming off of the ground

## Key light
Contributed by Taylor

The game takes place at an outdoor scene where the main lighting should be the sun.

Posible future design:
There is also the potential to bring the game to a night scene as well, where the key light might come from a darker blue-ish moonlight effect.

## Fill light
Contributed by Steffen

Each tower design would have a form of source lighting coming from within the towers. This would be crucial lighting during a night scene but it may not be entirely necessary during a day scene. The light would not only illuminate the inner parts of the tower model, but also the surrounding towers and creatures.

## Rim light
Contributed by Jackson

The final lighting will come from the ground. This effect comes from the reflection of the key and fill lights as well as some emision shaders.

# Shaders

Each of us made a custom animation shader to create a water effect, a tree blowing in the wind effect, and a creature walking animation.

## Water animation
Contributed by Jackson

The water animation is created by a waving animated plane going through the ground in some areas. The terrain is restructured to have dips below water level to have the water animation displayed

## Breaze animation
Contributed by Taylor

The breaze of rustling trees make the game feel more realistic. This shader animates the tree leaves to wave about, giving it a breaze effect.

## Creature animation
Contributed by Steffen

The creatures are designed as ink figures on paper. They were given life and are able to move, but it is still restricted by the paper it was drawn in. This effect brings that life to the creatures. Using a vertex shader to ossilate the walking animation makes it look much better.

Note how the shadows follow the animation.
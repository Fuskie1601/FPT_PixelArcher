STAT SYSTEM:

+CREDIT :
    -Modified Stat System from Kryzarel
    -Link to original : https://forum.unity.com/threads/tutorial-character-stats-aka-attributes-system.504095/

+OVERVIEW:
    -System to manage character stats with Composite Pattern
    -Allow modification of stat with flat addition , multiplication and additive multiplication without calculation error
    -Check code and comment for implementation detail

+INSTRUCTION:
    -Instantiate a CharacterStat objects as desire stat with base value (i.e : CharacterStat damage = new CharacterStat(5);)
    -To add modifier Instantiate a StatModifier object then add them in CharacterStat object with AddModifier()
    -To remove modifier use RemoveModifier() with the same StatModifier , remember cache it to remove it later
    -To get value call Value

+METHOD OVERVIEW:
    -AddModifier(StatModifier mod);
        - Add a StatModifier in CharacterStat's List , change IsDirty = true to recalculate the value when get value
    -RemoveModifier(StatModifier mod);
        - Remove a StatModifier in CharacterStat's List , change IsDirty = true to recalculate the value when get value
    -Value;
        - Get Stat's value , will recalculate when IsDirty == true

+TO-DO:
    -Add more functionality when needed in the future
    -CharacterStat quick AddModifier() method

+CHANGES LOGS:
    -25/1/2024 : Created - Q

+MORE RESOURCE:
    - Attributes system with Composite pattern overview : 
        -https://code.tutsplus.com/using-the-composite-design-pattern-for-an-rpg-attributes-system--gamedev-243t
    

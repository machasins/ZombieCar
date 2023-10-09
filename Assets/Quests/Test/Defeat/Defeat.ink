VAR char_name = "Distressed David"
VAR char_sprite = "test_sad"

VAR quest_2 = false
VAR quest_1 = false
VAR quest_0 = false

{quest_2: -> finished}
{quest_1: -> defeated}
{quest_0: -> accepted}

- Hey you! Yeah, you!
- I need help! <b>Danger Mc DoomCar</b> is harrassing out city! Will you help?
* [Of course.]
    Thank you! Thank you, kind sir! <>
* [I'm busy.]
    Please, I beg of you! <>
- <b>Danger Mc DoomCar</b> is just to the north!
~ quest_0 = true
-> END

=== accepted ===
- Please defeat the evil <b>Danger Mc DoomCar</b>! They're just to the north! Please!
-> END

=== defeated ===
~ char_sprite = "test_happy"
- Oh, thank you! You know not the pain you have alleved.
- Please, this is all I can offer you in return.
~ quest_2 = true
-> END

=== finished ===
~ char_sprite = "test_happy"
- Thank you again, hero!
-> END
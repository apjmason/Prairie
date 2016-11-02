# Code Standard

## Whitespace
Use tabs instead of spaces (MonoDevelop default).

Any curly brace defining control flow, scope, or the bounds of a implementation goes on its own line.

Do not use single line if statements.

	if (this.enabled)
	{
		// …
	}
	else
	{
		// …
	}

## Variables, Parameters, Fields, Properties
All named values should be given descriptive names which indicate their use clear enough that one can understand their role in the program without detailed inspection. Favor clear names which state the purpose of the variable over short names.

	GameObject trigger; // how do we use this? what does it represent?
	GameObject lastInteractor; // much clearer

Common words can be abbreviated, such as “temp“ for “temporary”. Variables used solely for iteration, or as parameters in pure mathematical functions can be given short names, such as “i”.

Variable, parameter, field, and property names use _lowerCamelCase_. Constant names use _SCREAMING_SNAKE_CASE_.

Define variables alongside their type. Do not use the `var` keyword.

	GameObject pivotTarget = GameObject.Find (“door”);
	for (int i=0; i < ATTEMPT_COUNT; i++)
	{
		// …
	}

## Methods
All methods have descriptive names which indicate their functionality and side effects clearly. Favor descriptive names over short ones.

Method names are written in _UpperCamelCase_.

A Get and Setter pair for a field should typically be replaced with a [C# property](https://msdn.microsoft.com/en-us/library/x9fsa0sw.aspx).

If a method has the potential to return `null`, specify it in the documentation.

A call to a method should have a space character separate the name of the method from the parameter list.

## Class Names
Name a class with clarity in what it does, or what data it represents.

If a class derives from an abstract class (such as `Interaction`), use the abstract class as a suffix (`SlideshowInteraction`).

Runtime script classes can often use verbs in their name (such as `TumbleInteraction`) but should refrain from verb phrases (such as `StartTumblingInteraction`).

Static classes defined solely for their static extension methods should have the suffix “Extensions” and a prefix which defines what they class they extend.

## Additional
When referencing a field, property or method of the current `this` object, use the keyword `this`. It is clearer where the variable or functionality comes from. Additionally, the `this` style is the only way to properly call an extension method.

	if (this.enabled)
	{
		this.SetPlayerIsFrozen (true);
	}

Use a `foreach` block in the place of an `for` block wherever possible. It is easier to read, and less prone to off-by-one errors.

Specify the access level of all code elements. Do not make elements public unless they are critical to other API or need to be directly edited by our users in the Unity Inspector. Use the `[HideInInspector]` annotation to hide public fields/properties from end users if it is not appropriate for them to edit them.

Use the C# documentation markup on all publicly defined code elements.

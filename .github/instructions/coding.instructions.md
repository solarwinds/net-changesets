---
applyTo: "**/*.go,**/*.cs,**/*.java,**/*.kt,**/*.py,**/*.js,**/*.ts,**/*.tsx"
---

# Coding Instructions

## General code style and readability

- Write code that is readable, understandable, and maintainable for future readers.
- Aim to create software that is not only functional but also readable, maintainable, and efficient throughout its lifecycle.
- Prioritize clarity to make reading, understanding, and modifying code easier.
- Adhere to established coding standards and write well-structured code to reduce errors.
- Regularly review and refactor code to improve structure, readability, and maintainability. Always leave the codebase cleaner than you found it.

## Naming conventions

- Choose names for variables, functions, and classes that reflect their purpose and behavior.
- A name should tell you why it exists, what it does, and how it is used. If a name requires a comment, then the name does not reveal its intent.
- Use specific names that provide a clearer understanding of what the variables represent and how they are used.

## DRY principle

- Follow the DRY (Don't Repeat Yourself) Principle and Avoid Duplicating Code or Logic.
- Avoid writing the same code more than once. Instead, reuse your code using functions, classes, modules, libraries, or other abstractions.
- Modify code in one place if you need to change or update it.

## Function length and responsibility

- Write short functions that only do one thing.
- Follow the single responsibility principle (SRP), which means that a function should have one purpose and perform it effectively.
- If a function becomes too long or complex, consider breaking it into smaller, more manageable functions.

## Comments usage

- Use comments sparingly, and when you do, make them meaningful.
- Don't comment on obvious things. Excessive or unclear comments can clutter the codebase and become outdated.
- Use comments to convey the "why" behind specific actions or explain unusual behavior and potential pitfalls.
- Provide meaningful information about the function's behavior and explain unusual behavior and potential pitfalls.

## Conditional encapsulation

- One way to improve the readability and clarity of functions is to encapsulate nested if/else statements into other functions.
- Encapsulating such logic into a function with a descriptive name clarifies its purpose and simplifies code comprehension.

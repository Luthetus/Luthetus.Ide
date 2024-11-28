I don't like having so many directories nested.

But, I want to have each ISymbol rendered by its own
component for the sake of sanity.

The SymbolDisplay is intended to not re-render
so optimization with static render fragments is not a concern.

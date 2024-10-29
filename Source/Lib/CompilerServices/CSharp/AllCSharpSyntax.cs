namespace Luthetus.CompilerServices.CSharp;

/// <summary></summary>
public class AllCSharpSyntax
{
	// Non-Contextual Keywords
	// =======================
    // abstract
    // as
    // base
    // bool
    // break
    // byte
    // case
    // catch
    // char
    // checked
    // class
    // const
    // continue
    // decimal
    // default
    // delegate
    // do
    // double
    // else
    // enum
    // event
    // explicit
    // extern
    // false
    // finally
    // fixed
    // float
    // for
    // foreach
    // goto
    // if
    // implicit
    // in
    // int
    // interface
    // internal
    // is
    // lock
    // long
    // namespace
    // new
    // null
    // object
    // operator
    // out
    // override
    // params
    // private
    // protected
    // public
    // readonly
    // ref
    // return
    // sbyte
    // sealed
    // short
    // sizeof
    // stackalloc
    // static
    // string
    // struct
    // switch
    // this
    // throw
    // true
    // try
    // typeof
    // uint
    // ulong
    // unchecked
    // unsafe
    // ushort
    // using
    // virtual
    // void
    // volatile
    // while

    // Contextual Keywords
    // ===================
    // add
    // and
    // alias
    // ascending
    // args
    // async
    // await
    // by
    // descending
    // dynamic
    // equals
    // file
    // from
    // get
    // global
    // group
    // init
    // into
    // join
    // let
    // managed
    // nameof
    // nint
    // not
    // notnull
    // nuint
    // on
    // or
    // orderby
    // partial
    // record
    // remove
    // required
    // scoped
    // select
    // set
    // unmanaged
    // value
    // var
    // when
    // where
    // with
    // yield
    
    // Access Modifier
    // ===============
	// Public
	// ProtectedInternal
	// Protected
	// Internal
	// PrivateProtected
	// Private

	// Storage Modifier
    // ================
	// Struct
    // Class
    // Interface
    // Enum
    // Record
    // RecordStruct
    
    // VariableKind
    // ============
	// Local
	// Field
	// Property
	
	/// <summary>
	/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/
	/// </summary>
	public void Operators()
	{
		// Primary;
		x.y; f(x); a[i]; x?.y; x?[y]; x++; x--; x!; new; typeof; checked; unchecked; default; nameof; delegate; sizeof; stackalloc; x->y;
		
		// Unary
		+x; -x; !x; ~x; ++x; --x; ^x; (T)x; await; &x; *x; true and false;
		
		// Range
		x..y;
		
		// switch and with expressions
		switch; with;
		
		// Multiplicative
		x * y; x / y; x % y;
		
		// Additive
		x + y; x – y;
		
		// Shift
		x << y; x >> y; x >>> y;
		
		// Relational and type-testing
		x < y; x > y; x <= y; x >= y; is; as;
		
		// Equality
		x == y; x != y;
		
		// Boolean logical AND or bitwise logical AND
		x & y;
		
		// Boolean logical XOR or bitwise logical XOR
		x ^ y;
		
		// Boolean logical OR or bitwise logical OR
		x | y;
		
		// Conditional AND
		x && y;
		
		// Conditional OR
		x || y;
		
		// Null-coalescing operator
		x ?? y;
		
		// Conditional operator
		c ? t : f
		
		// Assignment and lambda declaration
		x = y; x += y; x -= y; x *= y; x /= y; x %= y; x &= y; x |= y; x ^= y; x <<= y; x >>= y; x >>>= y; x ??= y; =>;
	}
}

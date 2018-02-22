namespace ZacharyKniebel.Foundation.Rules.Operators.ItemHierarchy
{
    /// <remarks>
    /// Credit to Matt Gramolini for originally developing the ItemHierarchy operator and this type. It
    /// has since been added to.
    /// </remarks>
	public enum ItemHierarchyType
	{
		Ancestors,
		AncestorsAndSelf,
		Children,
		ChildrenAndSelf,
		Descendants,
		DescendantsAndSelf,
		Parent,
		ParentAndSelf,
		Self
	}
}
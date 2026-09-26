using AwesomeAssertions;
using GodotGadgets.UI.Pagination;

namespace GadgetsTests.UI.PaginationSpecs;

public class PaginationStateSpecs
{
    const int DefaultPageSize = 10;

    [Test]
    public void initial_state_starts_at_first_page_with_no_data()
    {
        var state = PaginationState.Initial(DefaultPageSize);

        state.PageIndex.Should().Be(0);
        state.TotalItemCount.Should().Be(0);
        state.TotalPages.Should().Be(0);
    }

    [Test]
    public void throws_when_page_size_is_not_positive()
    {
        var createFn = () => PaginationState.Initial(0);

        createFn.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void total_pages_rounds_up()
    {
        new PaginationState(0, 10, 25).TotalPages.Should().Be(3);
        new PaginationState(0, 10, 20).TotalPages.Should().Be(2); // 边界：刚好整除
        new PaginationState(0, 10, 21).TotalPages.Should().Be(3); // 边界：多一条就要多一页
        new PaginationState(0, 10, 0).TotalPages.Should().Be(0);
    }

    [Test]
    public void default_struct_has_no_pages_instead_of_dividing_by_zero()
    {
        default(PaginationState).TotalPages.Should().Be(0);
    }

    [Test]
    public void empty_data_cannot_navigate()
    {
        var state = PaginationState.Initial(DefaultPageSize);

        state.HasPreviousPage.Should().BeFalse();
        state.HasNextPage.Should().BeFalse();
        state.Go(new PageNav.Next()).Should().Be(state);
        state.Go(new PageNav.Last()).Should().Be(state);
    }

    [Test]
    public void go_next_and_previous_move_one_page()
    {
        var middle = new PaginationState(1, 10, 50);

        middle.Go(new PageNav.Next()).PageIndex.Should().Be(2);
        middle.Go(new PageNav.Previous()).PageIndex.Should().Be(0);
    }

    [Test]
    public void go_first_is_noop_on_first_page()
    {
        var first = new PaginationState(0, 10, 50);

        first.Go(new PageNav.First()).Should().Be(first);
    }

    [Test]
    public void go_last_moves_to_last_page()
    {
        new PaginationState(0, 10, 25).Go(new PageNav.Last()).PageIndex.Should().Be(2);
    }

    [Test]
    [Arguments(-1, 0)]
    [Arguments(99, 4)]
    public void go_to_clamps_into_valid_range(int requested, int expected)
    {
        new PaginationState(0, 10, 50).Go(new PageNav.To(requested)).PageIndex.Should().Be(expected);
    }

    [Test]
    public void repeated_next_stops_at_last_page()
    {
        var state = new PaginationState(0, 10, 25);

        for (var i = 0; i < 5; i++) state = state.Go(new PageNav.Next());

        state.PageIndex.Should().Be(2);
    }

    [Test]
    public void current_request_carries_page_size_and_current_page()
    {
        new PaginationState(2, 10, 50).CurrentRequest.Should().Be(new PageRequest(2, 10));
    }

    [Test]
    public void view_data_of_empty_data_shows_first_page_of_zero()
    {
        var viewData = PaginationState.Initial(DefaultPageSize).ToViewData();

        viewData.PageNumber.Should().Be(1);
        viewData.TotalPages.Should().Be(0);
        viewData.HasPreviousPage.Should().BeFalse();
        viewData.HasNextPage.Should().BeFalse();
    }

    [Test]
    public void view_data_uses_one_based_page_number()
    {
        var viewData = new PaginationState(1, 10, 25).ToViewData();

        viewData.PageNumber.Should().Be(2);
        viewData.TotalPages.Should().Be(3);
        viewData.HasPreviousPage.Should().BeTrue();
        viewData.HasNextPage.Should().BeTrue();
    }

    [Test]
    public void view_data_marks_last_page_as_having_no_next()
    {
        var viewData = new PaginationState(2, 10, 25).ToViewData();

        viewData.PageNumber.Should().Be(3);
        viewData.HasNextPage.Should().BeFalse();
    }

    [Test]
    public void with_total_item_count_writes_total_and_keeps_page()
    {
        var loaded = PaginationState.Initial(DefaultPageSize).WithTotalItemCount(25);

        loaded.TotalItemCount.Should().Be(25);
        loaded.TotalPages.Should().Be(3);
        loaded.PageIndex.Should().Be(0);
    }

    [Test]
    public void with_total_item_count_keeps_index_when_still_in_range()
    {
        var loaded = new PaginationState(1, 10, 50).WithTotalItemCount(30);

        loaded.PageIndex.Should().Be(1);
    }

    [Test]
    public void with_total_item_count_normalizes_index_when_total_shrinks()
    {
        // 取数前用旧的 total 算出"第 5 页"合法；取数后总数缩水到 21 条（3 页）⇒ 页码必须归一化回 2
        var stale = new PaginationState(4, 10, 50);

        var loaded = stale.WithTotalItemCount(21);

        loaded.TotalPages.Should().Be(3);
        loaded.PageIndex.Should().Be(2);
    }

    [Test]
    [Arguments(1, 3, 2)] // 教程页：每页 1 条，共 3 条 ⇒ 3 页，末页 index 2
    [Arguments(2, 5, 2)] // 历史页：每页 2 条，共 5 条 ⇒ 3 页，末页 index 2
    public void navigates_with_real_consumer_page_sizes(int pageSize, int totalItemCount, int expectedLastIndex)
    {
        var state = new PaginationState(0, pageSize, totalItemCount);

        state.HasPreviousPage.Should().BeFalse();
        state.HasNextPage.Should().BeTrue();
        state.Go(new PageNav.Last()).PageIndex.Should().Be(expectedLastIndex);
    }
}

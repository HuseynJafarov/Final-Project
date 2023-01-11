$(function () {

	"use strict";


	$(document).on("click", ".add-to-cart", function () {

		let productId = parseInt($(this).closest(".product-item").children(0).val());

		let data = { id: productId };


		$.ajax({
			url: "/home/AddBasket",
			type: "POST",
			data: data,
			contentType: "application/x-www-form-urlencoded",
			success: function (res) {

				new Swal({

					position: 'mid',
					type: 'success',
					icon: 'success',
					title: 'Product Added Basket',
					showConfirmButton: false,
					timer: 1000

				});
			}
		})
	});



	$(document).on("click", ".add-to-wish", function () {

		let productId = parseInt($(this).closest(".product-item").children(0).val());

		let data = { id: productId };


		$.ajax({
			url: "/home/AddWish",
			type: "POST",
			data: data,
			contentType: "application/x-www-form-urlencoded",
			success: function (res) {

				new Swal({

					position: 'mid',
					type: 'success',
					icon: 'success',
					title: 'Product Added Wishlish',
					showConfirmButton: false,
					timer: 1000

				});
			}
		})
	});

	/////////////////////////////////Search
	$(document).on("keyup", "#searchinp", function () {
		let inputVal = $(this).val().trim();
		$(".search-list-p li").slice(0).remove();
		$.ajax({
			method: "get",
			url: "home/search?search=" + inputVal,
			success: function (res) {
				$(".search-list-p").append(res);
			}
		});
	});
	/////////////////////////////////Search


	////////category

	$(document).on('click', '.categories', function (e) {
		e.preventDefault();
		$(this).next().next().slideToggle();
	})

	$(document).on('click', '.category li a', function (e) {
		e.preventDefault();
		let category = $(this).attr('data-id');
		let products = $('.product-item ');
		let paginate = $('page-item');
		products.each(function () {
			if (category == $(this).attr('data-id')) {
				$(this).parent().fadeIn();
			}
			else {
				$(this).parent().hide();
			}
		})
		if (category == 'all') {
			products.parent().fadeIn();
		}
	})

	////////Category


	//load more
	$(document).on("click", ".show-more button", function () {

		let parent = $(".parent-products");
		let skipCount = $(".parent-products").children().length;
		let productCount = $(".product-count").val();


		$.ajax({
			url: "/home/loadmore",
			type: "get",
			data: {
				skip: skipCount
			},
			success: function (res) {

				$(parent).append(res);
				skipCount = $(".parent-products").children().length;

				if (skipCount >= productCount) {
					$(".show-more button").addClass("d-none");
				}

			}
		})
	}); 

	//load more








});


	var swiper = new Swiper(".mySwiper", { });



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






});


	var swiper = new Swiper(".mySwiper", { });



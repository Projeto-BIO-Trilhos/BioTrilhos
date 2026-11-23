<?php

namespace App\Http\Controllers;

use Illuminate\Support\Facades\DB;

class EstufaController extends Controller
{
    public function index()
    {
        return response()->json(DB::table('estufas')->get());
    }
}

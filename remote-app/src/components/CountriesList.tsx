import React, { useEffect, useState } from "react";
import axios from "axios";

interface Country {
    id: number;
    name: string;
}

const CountriesList: React.FC = () => {
    const [countries, setCountries] = useState<Country[]>([]);

    useEffect(() => {
        axios.get<Country[]>("http://localhost:5263/api/countries")
            .then(res => setCountries(res.data))
            .catch(err => console.error(err));
    }, []);

    return (
        <div>
            <h2>Countries</h2>
            <ul>
                {countries.map(c => (
                    <li key={c.id}>{c.name}</li>
                ))}
            </ul>
        </div>
    );
};

export default CountriesList;